echo "PATH ======= $PATH"

cd .. && mv s xamarin-macios && cd xamarin-macios
make $1
cd .. && mv xamarin-macios s && cd s

