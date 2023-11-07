TOP=../../..
WEBSITE_DOCS ?= $(TOP)/xamarin-docs-pr

all:
	cp mtouch-errors.md $(WEBSITE_DOCS)/docs/ios/troubleshooting/mtouch-errors.md
	cp mmp-errors.md $(WEBSITE_DOCS)/docs/mac/troubleshooting/mmp-errors.md
	cp binding_objc_libs.md $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/objective-c-libraries.md
	cp binding_types_reference_guide.md $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/binding-types-reference.md
	cp optimizations.md $(WEBSITE_DOCS)/docs/cross-platform/macios/optimizations.md

diff:
	-diff -u $(WEBSITE_DOCS)/docs/ios/troubleshooting/mtouch-errors.md mtouch-errors.md
	-diff -u $(WEBSITE_DOCS)/docs/mac/troubleshooting/mmp-errors.md mmp-errors.md
	-diff -u $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/objective-c-libraries.md binding_objc_libs.md
	-diff -u $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/binding-types-reference.md binding_types_reference_guide.md
	-diff -u $(WEBSITE_DOCS)/docs/cross-platform/macios/optimizations.md optimizations.md
