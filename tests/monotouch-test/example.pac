// Exmaple PAC file that returns a proxy for the urls that have the
// xamarin domain name.
function FindProxyForURL(url, host) {
	if (dnsDomainIs(host, "xamarin.com"))
		return "PROXY example.com:8080";
	 
	if (dnsDomainIs(host, "google.com"))
		return "DIRECT";
}	 