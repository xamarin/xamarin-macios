/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 *  Authors: Miguel de Icaza
 *
 *  Copyright (C) 2015 Xamarin Inc. (www.xamarin.com)
 *
 */

#include <stdio.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#include <Foundation/Foundation.h>

#include <pthread.h>

// Silence this warning:
// nsstring-localization.m:22:46: warning: format string is not a string literal (potentially insecure) [-Wformat-security]
//        return [NSString localizedStringWithFormat: format];
#pragma clang diagnostic ignored "-Wformat-security"

extern "C" {

void *
xamarin_localized_string_format (NSString *format)
{
	return [NSString localizedStringWithFormat: format];
}

void *
xamarin_localized_string_format_1 (NSString *format, id a)
{
	return [NSString localizedStringWithFormat: format, a];
}

void *
xamarin_localized_string_format_2 (NSString *format, id a, id b)
{
	return [NSString localizedStringWithFormat: format, a, b];
}

void *
xamarin_localized_string_format_3 (NSString *format, id a, id b, id c)
{
	return [NSString localizedStringWithFormat: format, a, b, c];
}

void *
xamarin_localized_string_format_4 (NSString *format, id a, id b, id c, id d)
{
	return [NSString localizedStringWithFormat: format, a, b, c, d];
}

void *
xamarin_localized_string_format_5 (NSString *format, id a, id b, id c, id d, id e)
{
	return [NSString localizedStringWithFormat: format, a, b, c, d, e];
}

void *
xamarin_localized_string_format_6 (NSString *format, id a, id b, id c, id d, id e, id f)
{
	return [NSString localizedStringWithFormat: format, a, b, c, d, e, f];
}

void *
xamarin_localized_string_format_7 (NSString *format, id a, id b, id c, id d, id e, id f, id g)
{
	return [NSString localizedStringWithFormat: format, a, b, c, d, e, f, g];
}

void *
xamarin_localized_string_format_8 (NSString *format, id a, id b, id c, id d, id e, id f, id g, id h)
{
	return [NSString localizedStringWithFormat: format, a, b, c, d, e, f, g, h];
}

void *
xamarin_localized_string_format_9 (NSString *format, id a, id b, id c, id d, id e, id f, id g, id h, id i)
{
	return [NSString localizedStringWithFormat: format, a, b, c, d, e, f, g, h, i];
}

}
