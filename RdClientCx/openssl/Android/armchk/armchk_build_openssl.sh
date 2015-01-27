#!/bin/bash
# Change NDK path below to your NDK directory
export NDK=/cygdrive/e/Android/NDK/android-ndk-r10c/
$NDK/build/tools/make-standalone-toolchain.sh --platform=android-15 --toolchain=arm-linux-androideabi-4.8 --install-dir=$PWD/android-toolchain-arm --system=windows-x86_64
chmod -R 777 android-toolchain-arm
export TOOLCHAIN_PATH=$PWD/android-toolchain-arm/bin
export TOOL=arm-linux-androideabi
export NDK_TOOLCHAIN_BASENAME=${TOOLCHAIN_PATH}/${TOOL}
export CC=$NDK_TOOLCHAIN_BASENAME-gcc
export CXX=$NDK_TOOLCHAIN_BASENAME-g++
export LINK=${CXX}
export LD=$NDK_TOOLCHAIN_BASENAME-ld
export AR=$NDK_TOOLCHAIN_BASENAME-ar
export RANLIB=$NDK_TOOLCHAIN_BASENAME-ranlib
export STRIP=$NDK_TOOLCHAIN_BASENAME-strip
export ARCH_FLAGS="-mthumb"
export ARCH_LINK=
export COMMON_LANG_FLAGS=" ${ARCH_FLAGS} -fpie -fpic -fdata-sections -ffunction-sections -funwind-tables -fvisibility=hidden -D_FORTIFY_SOURCE=2 -fstack-protector-all -fno-strict-aliasing -finline-limit=64 -O0 -ggdb -DNDK_DEBUG=1 -DDEBUG -DCONF_DEBUG -DBN_DEBUG -DOPENSSL_NO_HEARTBEATS "
export CPPFLAGS=" ${COMMON_LANG_FLAGS} "
export CFLAGS=" ${COMMON_LANG_FLAGS} "
export CXXFLAGS=" ${COMMON_LANG_FLAGS} -frtti "
export LDFLAGS=" ${ARCH_LINK} "
./Configure no-ssl2 no-shared no-asm android

make depend -j7

PATH=$TOOLCHAIN_PATH:$PATH make -j7
