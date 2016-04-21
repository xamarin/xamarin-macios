#!/usr/bin/perl
#
# Remove "extra" enum values from the GLEnum.cs file.
#
# syntax: cat GLEnum.cs | rm-extra-enums.pl keep.txt > GLEnums.iPhone.cs
#
use strict;

my $keep = shift;
unless (defined $keep) {
  print "usage: $0 <keep>\n";
  exit 1;
}

my %keep = get_constants_to_keep ($keep);
my $re = '\b' . join ('\b|\b', values %keep) . '\b';
my %seen = ();
foreach my $k (values %keep) { $seen{lc($k)} = 0; }

while (<STDIN>) {
  last if /^}/;

  my ($n, $v);
  unless (($n, $v) = /^\s+(\w+) = \(\(int\)(\w+)\),/) {
    print;
  }
  else {
    my @matches = grep m/$re/i, $_;
    if (scalar @matches) {
      print;
      $seen{lc($v)} = 1;
    }
  }
}

print "\n";
print "    //\n";
print "    // iPhone OS OpenGL ES Additions\n";
print "    //\n";
print "    public enum iPhoneAdditions : int {\n";
foreach my $n (keys %keep) {
  my $v = lc($keep{$n});
  next if $seen{$v};
  my $name = get_value_name ($n);
  print "        $name = ((int)$v),\n";
}
print "    }\n";
print "}\n";

sub get_constants_to_keep {
  my $extra = shift;

  open(my $fh, "<", $extra) or die "Cannot open $extra!";

  my %extra = ();
  while (<$fh>) {
    chomp;
    my ($n, $v);
	  next unless ($n, $v) = /^(\w+) (\w+)$/;
    $extra{$n} = $v;
  }

  close $fh;

  return %extra;
}

sub get_value_name {
  my $name = shift;

  $name =~ s/^GL//;
  $name =~ tr[A-Z][a-z];
  $name =~ s/_(.)/\u\1/g;

  return $name;
}

# vim: et ts=2 sw=2
