var ajax_log = null;

function addlog (msg)
{
	if (ajax_log == null)
		ajax_log = document.getElementById ('ajax-log');
	if (ajax_log == null)
		return;
	var newText = msg + "\n" + ajax_log.innerText;
	if (newText.length > 1024)
		newText = newText.substring (0, 1024);
	ajax_log.innerText = newText;
}

function toggleLogVisibility (logName)
{
	var button = document.getElementById ('button_' + logName);
	var logs = document.getElementById ('logs_' + logName);
	if (logs.style.display == 'none' && logs.innerText.trim () != '') {
		logs.style.display = 'block';
		button.innerText = '-';
	} else {
		logs.style.display = 'none';
		button.innerText = '+';
	}
}

function toggleContainerVisibility2 (containerName)
{
	var button = document.getElementById ('button_container2_' + containerName);
	var div = document.getElementById ('test_container2_' + containerName);
	if (div.style.display == 'none') {
		div.style.display = 'block';
		button.innerText = '-';
	} else {
		div.style.display = 'none';
		button.innerText = '+';
	}
}

function quit ()
{
	var xhttp = new XMLHttpRequest();
	xhttp.onreadystatechange = function() {
		if (this.readyState == 4 && this.status == 200) {
		   window.close ();
		}
	};
	xhttp.open("GET", "quit", true);
	xhttp.send();
}

function toggleAjaxLogVisibility()
{
	if (ajax_log == null)
		ajax_log = document.getElementById ('ajax-log');
	var button = document.getElementById ('ajax-log-button');
	if (ajax_log.style.display == 'none') {
		ajax_log.style.display = 'block';
		button.innerText = 'Hide log';
	} else {
		ajax_log.style.display = 'none';
		button.innerText = 'Show log';
	}
}

function toggleVisibility (css_class)
{
	var objs = document.getElementsByClassName (css_class);

	for (var i = 0; i < objs.length; i++) {
		var obj = objs [i];

		var pname = 'original-' + css_class + '-display';
		if (obj.hasOwnProperty (pname)) {
			obj.style.display = obj [pname];
			delete obj [pname];
		} else {
			obj [pname] = obj.style.display;
			obj.style.display = 'none';
		}
	}
}

function keyhandler(event)
{
	switch (String.fromCharCode (event.keyCode)) {
	case "q":
	case "Q":
		quit ();
		break;
	}
}

function runalltests()
{
	sendrequest ("runalltests");
}

function runtest(id)
{
	sendrequest ("runtest?id=" + id);
}

function stoptest(id)
{
	sendrequest ("stoptest?id=" + id);
}

function sendrequest(url, callback)
{
	var xhttp = new XMLHttpRequest();
	xhttp.onreadystatechange = function() {
		if (this.readyState == 4) {
			addlog ("Loaded url: " + url + " with status code: " + this.status + "\nResponse: " + this.responseText);
			if (callback)
				callback (this.responseText);
		}
	};
	xhttp.open("GET", url, true);
	xhttp.send();
	addlog ("Loading url: " + url);
}

function autorefresh()
{
	var xhttp = new XMLHttpRequest();
	xhttp.onreadystatechange = function() {
		if (this.readyState == 4) {
			addlog ("Reloaded.");
			var parser = new DOMParser ();
			var r = parser.parseFromString (this.responseText, 'text/html');
			var ar_objs = document.getElementsByClassName ("autorefreshable");

			for (var i = 0; i < ar_objs.length; i++) {
				var ar_obj = ar_objs [i];
				if (!ar_obj.id || ar_obj.id.length == 0) {
					console.log ("Found object without id");
					continue;
				}

				var new_obj = r.getElementById (ar_obj.id);
				if (new_obj) {
					if (ar_obj.innerHTML != new_obj.innerHTML)
						ar_obj.innerHTML = new_obj.innerHTML;
					if (ar_obj.style.cssText != new_obj.style.cssText) {
						ar_obj.style = new_obj.style;
					}

					var evt = ar_obj.getAttribute ('data-onautorefresh');
					if (evt != '') {
						autoshowdetailsmessage (evt);
					}
				} else {
					console.log ("Could not find id " + ar_obj.id + " in updated page.");
				}
			}
			setTimeout (autorefresh, 1000);
		}
	};
	xhttp.open("GET", window.location.href, true);
	xhttp.send();
}

function autoshowdetailsmessage (id)
{
	var input_id = 'logs_' + id;
	var message_id = 'button_' + id;
	var input_div = document.getElementById (input_id);
	if (input_div == null)
		return;
	var message_div = document.getElementById (message_id);
	var txt = input_div.innerText.trim ();
	if (txt == '') {
		message_div.style.opacity = 0;
	} else {
		message_div.style.opacity = 1;
		if (input_div.style.display == 'block') {
			message_div.innerText = '-';
		} else {
			message_div.innerText = '+';
		}
	}
}

function oninitialload ()
{
	var autorefreshable = document.getElementsByClassName ("autorefreshable");
	for (var i = 0; i < autorefreshable.length; i++) {
		var evt = autorefreshable [i].getAttribute ("data-onautorefresh");
		if (evt != '')
			autoshowdetailsmessage (evt);
	}
}

function toggleAll (show)
{
	var expandable = document.getElementsByClassName ('expander');
	var counter = 0;
	var value = show ? '-' : '+';
	for (var i = 0; i < expandable.length; i++) {
		var div = expandable [i];
		if (div.textContent != value)
			div.textContent = value;
		counter++;
	}

	var togglable = document.getElementsByClassName ('togglable');
	counter = 0;
	value = show ? 'block' : 'none';
	for (var i = 0; i < togglable.length; i++) {
		var div = togglable [i];
		if (div.style.display != value) {
			if (show && div.innerText.trim () == '') {
				// don't show nothing
			} else {
				div.style.display = value;
			}
		}
		counter++;
	}
}
