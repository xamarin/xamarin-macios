Platform assemblies
===================

This directory contains the source code and build logic to build the platform assemblies.

Conditional compilation
=======================

These are the symbols defined for each platform assembly:

| Assembly            | Symbols                                        |
| ------------------  | -----------                                    |
| monotouch.dll       | IPHONE MONOTOUCH IOS                           |
| Xamarin.iOS.dll     | IPHONE MONOTOUCH IOS XAMCORE_2_0               |
| XamMac.dll          | MONOMAC XAMARIN_MAC                            |
| Xamarin.Mac.dll     | MONOMAC XAMARIN_MAC XAMCORE_2_0                |
| Xamarin.WatchOS.dll | IPHONE MONOTOUCH WATCH XAMCORE_2_0 XAMCORE_3_0 |
| Xamarin.TVOS.dll    | IPHONE MONOTOUCH TVOS XAMCORE_2_0 XAMCORE_3_0  |

To build core for only one platform, use the platform unique variables `IOS`, `MONOMAC`, `WATCH` or `TVOS`.
