
#
# OpenTK[-1.0]
#
IOS_OPENTK_1_0_CORE_SOURCES = \
	$(OPENTK_PATH)/Source/OpenTK/Math/BezierCurve.cs			\
	$(OPENTK_PATH)/Source/OpenTK/Math/BezierCurveCubic.cs		\
	$(OPENTK_PATH)/Source/OpenTK/Math/BezierCurveQuadric.cs		\
	$(OPENTK_PATH)/Source/OpenTK/Math/Box2.cs					\
	$(OPENTK_PATH)/Source/OpenTK/Math/Functions.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Half.cs					\
	$(OPENTK_PATH)/Source/OpenTK/Math/MathHelper.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Matrix2.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Matrix3.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Matrix3d.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Matrix4.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Matrix4d.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Quaternion.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Quaterniond.cs			\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector2.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector2d.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector2h.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector3.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector3d.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector3h.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector4.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector4d.cs				\
	$(OPENTK_PATH)/Source/OpenTK/Math/Vector4h.cs				\
	OpenGLES/OpenTK/Math/Vector2i.cs					\
	OpenGLES/OpenTK/Math/Vector3i.cs					\
	OpenGLES/OpenTK/Math/Vector4i.cs					\

# Xamarin.iOS

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0%: $(MACIOS_BINARIES_PATH)/OpenTK/ios/OpenTK-1.0%
	$(Q) $(CP) $< $@

# Xamarin.TVOS

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0%: $(MACIOS_BINARIES_PATH)/OpenTK/tvos/OpenTK-1.0%
	$(Q) $(CP) $< $@

# Xamarin.Mac

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/%: $(MACIOS_BINARIES_PATH)/OpenTK/macos/lib/%
	$(Q) mkdir -p $(dir $@)
	$(Q) $(CP) $^ $@

MACOS_TARGETS += \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/OpenTK.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/OpenTK.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile/OpenTK.dll        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile/OpenTK.pdb        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/OpenTK.dll       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/OpenTK.pdb       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full/OpenTK.dll          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full/OpenTK.pdb          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/OpenTK.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/OpenTK.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/net_4_5/OpenTK.dll    \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/net_4_5/OpenTK.pdb	   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/OpenTK.dll             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/OpenTK.pdb             \
