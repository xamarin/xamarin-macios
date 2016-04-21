print "<html><head><title>MonoTouch Rosetta Stone</title><link rel='stylesheet' href='global.css' type='text/css' /></head><body>";

while (<>){
    print "<h1>Namespace $1</h1>\n" if $_ =~ /namespace(.*){/; 
    print "<h3>Class $1</h3>\n" if $_ =~ /interface (.*){/; 
    
    if (/Export/){ 
        ($sel) = $_ =~ /"(.*)"/; 
        chop; 
        $a = <>; 
        chop $a; 
        $a =~ s/^[ \t]*//; 
	if ($a =~ /get;|set;/){
	    $t = "Property:";
	} else {
	    $t = "Method:";
	}
        print "\t<div class='selector'>Selector: <b><tt>$sel</tt></b>\t<br>\n\t$t <b><tt>$a</tt></b></div>\n";
    }
}
print "</body></html>";
