echo "PATH ======= $PATH"
if [ "$1" = "world" ]; then 
    cd .. && mv s xamarin-macios && cd xamarin-macios
fi
make $1
