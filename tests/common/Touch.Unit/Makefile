MDTOOL="/Applications/Xamarin Studio.app/Contents/MacOS/mdtool"
MTOUCH=/Developer/MonoTouch/usr/bin/mtouch
TOUCH_SERVER=./Touch.Server/bin/Debug/Touch.Server.exe

all: build-simulator build-device

run run-test: run-simulator run-device

$(TOUCH_SERVER):
	cd Touch.Server && xbuild

build-simulator:
	$(MDTOOL) -v build -t:Build "-c:Debug|iPhoneSimulator" Touch.Unit.sln

run-simulator: build-simulator Touch.Server
	rm -f sim-results.log
	mono --debug $(TOUCH_SERVER) --launchsim bin/iPhoneSimulator/Debug/TouchUnit.app -autoexit -logfile=sim-results.log
	cat sim-results.log

build-device:
	$(MDTOOL) -v build -t:Build "-c:Release|iPhone" Touch.Unit.sln

run-device: build-device
	$(MTOUCH) --installdev bin/iPhone/Release/TouchUnit.app
	# kill an existing instance (based on the bundle id)
	$(MTOUCH) --killdev com.xamarin.touch-unit
	rm -f dev-results.log
	mono --debug $(TOUCH_SERVER) --launchdev com.xamarin.touch-unit -autoexit -logfile=dev-results.log
	cat dev-results.log
