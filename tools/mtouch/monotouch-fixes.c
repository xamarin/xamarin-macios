//
// monotouch-fixes.c: 
// 
// Author:
//   Rolf Bjarne Kvinge
//
// Copyright 2011 Xamarin Inc.
//

#include <stdio.h>
#include <dlfcn.h>
#include <signal.h>
#include <sys/mman.h>
#include <string.h>
#include <unistd.h>
#include <stdint.h>
#include <errno.h>

// Apple uses LLVM to implement OpenGL ES on the iPhone Simulator, unfortunately
// LLVM has a race condition in their signal handling - they set one-shot
// signal handlers and restore the previous handler in those handlers. The
// problem is that they set SA_RESETHAND|SA_NODEFER when setting their signal
// handler, which means that if another signal arrives before their signal handler
// has been able to restore our signal handler, the default signal handler will
// be called (which just exits the process without any diagnostics at all).
//
// For the 32bit simulator we inject our own sigaction method, which takes precedence
// over the system one, and reset the flags to 0 if flags is SA_RESETHAND|SA_NODEFER.
//
// Unfortunately this technique does not seem to work for the 64bit simulator, so there
// we patch the system sigaction instead.
//

int my_sigaction (int signo, const struct sigaction *__restrict act, struct sigaction *__restrict oact);

typedef int (*SigAction) (int sig, const struct sigaction *__restrict act, struct sigaction *__restrict oact);
SigAction system_sigaction = sigaction;

int
my_sigaction (int sig, const struct sigaction *__restrict act, struct sigaction *__restrict oact)
{
	//fprintf (stderr, "my_sigaction (%i, %p: handler = %p mask = %i flags = %i, %p)\n", sig, act, act ? act->sa_handler : NULL, act ? act->sa_mask : -1, act ? act->sa_flags : -1, oact);

	if (act && act->sa_flags == (SA_NODEFER | SA_RESETHAND)) {
		switch (sig) {
		case SIGUSR1:
		case SIGUSR2:
		case SIGFPE:
			// fprintf (stderr, "fixed up sicaction (%i, %p: handler = %p mask = %i flags = %i, %p)\n", sig, act, act ? act->sa_handler : NULL, act ? act->sa_mask : -1, act ? act->sa_flags : -1, oact);
			((struct sigaction *) act)->sa_flags = 0;
			break;
		}
	}

	return system_sigaction (sig, act, oact);
}

#if defined(__i386__)

typedef struct interpose_s {
	void *new_func;
	void *orig_func;
} interpose_t;
 
static const interpose_t interposers[] __attribute__ ((unused)) \
	__attribute__ ((section("__DATA, __interpose"))) = { 
		{ (void *) &my_sigaction,  (void *) &sigaction  },
    };

#elif defined(__x86_64__)

void patch_sigaction ()
{
	// Sanity check.
	uint64_t * func = (uint64_t *) &sigaction;
	uint64_t first = 0x20ec8348e5894855; // first 8 bytes of sigaction
	uint64_t second = 0x0a771ef883ff478d; // second 8 bytes of sigaction
	if (func [0] != first || func [1] != second) {
		fprintf (stderr, "MonoTouch: Could not install sigaction override, unexpected sigaction implementation.\n");
		return;
	}

	// allocate executable memory
	uint64_t pagesize = getpagesize ();
	void *exec = mmap (NULL, pagesize, PROT_EXEC | PROT_WRITE | PROT_READ, MAP_ANON | MAP_PRIVATE, -1, 0);
	if (exec == NULL) {
		fprintf (stderr, "MonoTouch: Could not allocate memory for sigaction override: %s\n", strerror (errno));
		return;
	}

	// create a trampoline that jumps to the real sigaction (to just after where we modified it).
	uint64_t *i64 = (uint64_t *) exec;
	i64 [0] = first;
	i64 [1] = second;
	uint16_t *i16 = (uint16_t *) exec;
	// movabsq &sigaction + 14,%r11
	i16 [7] = 0xbb49;
	i64 [2] = (uint64_t) (14 + (char *) &sigaction);
	// jmpq *%r11
	i16 [12] = 0xff41;
	i16 [13] = 0x00e3;

	// make sigaction patchable.
	uint64_t symbol = (uint64_t) (char *) &sigaction;
	void *aligned_symbol = (void *) (symbol & ~(pagesize - 1));
	int ret;

	ret = mprotect (aligned_symbol, pagesize * 2, PROT_READ | PROT_WRITE | PROT_EXEC);

	if (ret != 0) {
		fprintf (stderr, "MonoTouch: could not patch sigaction: %s\n", strerror (errno));
		munmap (exec, pagesize);
		return;
	}

	// patch sigaction to call my_sigaction instead.
	uint16_t *f16 = (uint16_t *) &sigaction;
	// movabsq my_sigaction,%r11
	f16 [0] = 0xbb49; 
	uint64_t *f64 = (uint64_t *) (f16 + 1);
	f64 [0] = (uint64_t) &my_sigaction; 
	// jmpq *%r11
	f16 [5] = 0xff41;
	f16 [6] = 0x00e3;

	// Make sigaction non-writeable again.
	mprotect (aligned_symbol, pagesize * 2, PROT_READ | PROT_EXEC);

	system_sigaction = (SigAction) exec;
}
#endif
