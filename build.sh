echo "PATH ======= $PATH"
if [ "$1" = "world" ]; then 
    cd .. && mv s xamarin-macios && cd xamarin-macios
    make world
    cd .. && mv xamarin-macios s && cd s
else
	make $1
fi

