#!/usr/bin/bash

declare -r cpp_arm="-mthumb"
declare -r ld_arm=""
declare -r cpp_armv7="-march=armv7-a -mfloat-abi=softfp -mfpu=vfpv3-d16"
declare -r ld_armv7="-march=armv7-a -Wl, --fix-cortex-a8"
declare -r cpp_chk="-O0 -ggdb -DNDK_DEBUG=1 -DDEBUG -DCONF_DEBUG -DBN_DEBUG"
declare -r cpp_fre="-Os -g0 -DNDEBUG"
# TODO Check if we can disable more unnecessary features
#declare -r configure_common="no-ssl2 no-ssl3 no-idea no-cast no-md2 no-sha0 no-whrlpool no-engine no-shared no-asm "
declare -r configure_common="no-ssl2 no-shared no-asm"
declare -r configure_arm="$configure_common android"
declare -r configure_armv7="$configure_common android-armv7"

declare -r make="make -j7"

set -e

usage() {
echo "Usage: $(basename $0) path/to/openssl.X.Y.z.tar.gz
    
    Build OpenSSL static libraries for Android on Windows.
    
    Process for generating static libraries:
    
    -2) In a command prompt do 'SETX CYGWIN winsymlinks:nativestrict'.
        This will turn on windows native symbolic links. Cygwin's workaround
        doesn't work properly and links are interpreted as actual files by the
        Android compiler.
    -1) Download and install cygwin with make (select in package manager).
    
    0) Download openssl from openssl.org.
    
    1) Open a cygwin terminal as administrator to avoid 'permission denied' errors.
    
    2) Create an empty directory and navigate into it
    
    3) Set environment variable to NDK: 'export NDK=/path/to/android-ndk'.
    
    4) Run this script.
    
    5) Copy everything in the output directory to your repository.
" 
}

# Setup and export environment variables for compilation
# $1 -- compiler flags
# $2 -- linker flags
setup_common_flags() {
	local -r cpp_flags="$1"; shift
	local -r ld_flags="$1"; shift
	export TOOLCHAIN_PATH=`pwd`/android-toolchain-arm/bin
	export TOOL=arm-linux-androideabi
	export NDK_TOOLCHAIN_BASENAME=${TOOLCHAIN_PATH}/${TOOL}
	export CC=$NDK_TOOLCHAIN_BASENAME-gcc
	export CXX=$NDK_TOOLCHAIN_BASENAME-g++
	export LINK=${CXX}
	export LD=$NDK_TOOLCHAIN_BASENAME-ld
	export AR=$NDK_TOOLCHAIN_BASENAME-ar
	export RANLIB=$NDK_TOOLCHAIN_BASENAME-ranlib
	export STRIP=$NDK_TOOLCHAIN_BASENAME-strip
	export COMMON_LANG_FLAGS="-fpic -fpie -fdata-sections -ffunction-sections -fvisibility=hidden -funwind-tables -fstack-protector-all -fomit-frame-pointer -fno-strict-aliasing -finline-limit=64 -D_FORTIFY_SOURCE=2 -DOPENSSL_NO_HEARTBEATS ${cpp_flags}"
	export CPPFLAGS="${COMMON_LANG_FLAGS}"
	export CFLAGS="${COMMON_LANG_FLAGS}"
	export CXXFLAGS="${COMMON_LANG_FLAGS} -frtti"
	export LDFLAGS="${ld_flags}"
}

# Configure and build OpenSSL for the given architecture and build type.
# $1 -- path to openssl source tar.gz
# $2 -- armv5 or armv7
# $3 -- debug or release
build() {
	local -r file="$1"; shift
	local -r arch="$1"; shift
	local -r build_type="$1"; shift
	local -r ld_flags="ld_$arch"
	local -r arch_flags="cpp_$arch"
	local -r build_type_flags="cpp_$build_type"
	local -r configure_args="configure_$arch"
	local -r build_dir="${arch}${build_type}"
	local -r out_dir="out/$build_dir"
	mkdir $build_dir
	mkdir -p $out_dir
	tar xzf $file -C $build_dir --strip-components=1
	setup_common_flags "${!arch_flags} ${!build_type_flags}" "${!ld_flags}"
	(cd $build_dir && ./configure ${!configure_args})
	(cd $build_dir && $make depend)
	(cd $build_dir && PATH=$TOOLCHAIN_PATH:$PATH $make build_crypto build_ssl)
	echo "Build ${arch} $build_type} success"
	cp $build_dir/libcrypto.a $build_dir/libssl.a $out_dir
	echo "Build $build_dir done"
}

main() {
	[[ $NDK == "" ]] && echo "Error: NDK not set!" && usage && exit 1
	[[ $# != 1 ]] && echo "Error: Only path to openssl.tar.gz argument allowed!" && usage && exit 1
	local -r param_file="$1"; shift
	[[ ! -e $param_file ]] && echo "Error: file '$param_file' does not exist!" && usage && exit 1
	
	echo "Building toolchain"
	$NDK/build/tools/make-standalone-toolchain.sh --platform=android-15 --toolchain=arm-linux-androideabi-4.8 --install-dir=`pwd`/android-toolchain-arm --system=windows-x86_64
	chmod -R 777 android-toolchain-arm
	
	# Build all supported combinations.
	echo "Building all configurations"
	build $param_file 'arm' 'chk'
	build $param_file 'arm' 'fre'
	build $param_file 'armv7' 'chk'
	build $param_file 'armv7' 'fre'
	
	# Header files for all architectures and build types look the same.
	echo "Copying headers"
	mkdir -p "out/include"
	cp -Lr "armv7fre/include/openssl" "out/include"
	
	echo "Built all architectures successfully"
}

main "$@"
