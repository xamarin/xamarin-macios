#!/bin/bash

cat ../../external/mono/mono/metadata/image.c | grep IGNORED_ASSEMBLY |grep -v define | cut -d ',' -f 2,3 | sed -e 's/^[[:space:]]*//' |  tr -d '"' > temp.txt

rm monoExclusionList.txt
cat temp.txt | while read line
do
   AssemblyName=`echo $line | cut -d ',' -f 1`
   GUID=`echo $line | cut -d ',' -f 2 | tr -d '[:space:]'`  
   case $AssemblyName in
   SYS_RT_INTEROP_RUNTIME_INFO)
   	echo System.Runtime.InteropServices.RuntimeInformation.dll:$GUID >> monoExclusionList.txt
   ;;
   SYS_GLOBALIZATION_EXT)
   	echo System.Globalization.Extensions.dll:$GUID >> monoExclusionList.txt
   ;;
   SYS_IO_COMPRESSION)
   	echo System.IO.Compression.dll:$GUID >> monoExclusionList.txt
   ;;
   SYS_NET_HTTP)
   	echo System.Net.Http.dll:$GUID >> monoExclusionList.txt
   ;;
   SYS_TEXT_ENC_CODEPAGES)
   	echo System.Text.Encoding.CodePages.dll:$GUID >> monoExclusionList.txt
   ;;
   SYS_REF_DISP_PROXY)
   	echo System.Reflection.DispatchProxy.dll:$GUID >> monoExclusionList.txt
   ;;
   SYS_VALUE_TUPLE)
   	echo System.ValueTuple.dll:$GUID >> monoExclusionList.txt
   ;;
   *)
   	echo "Unknown name: $AssemblyName"
	exit 1
   ;;
   esac
done

rm temp.txt
