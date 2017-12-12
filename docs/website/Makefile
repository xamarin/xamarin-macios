TOP=../../..
WEBSITE_DOCS ?= $(TOP)/documentation
XAMARIN_ANALYSIS ?=$(TOP)/xamarin-analysis/shared/Xamarin.Analysis
XAMARIN_ANALYSIS_DOC = xamarin-analysis-doc-tool/bin/Debug

all: xamarin-analysis-doc
	cp mtouch-errors.md $(WEBSITE_DOCS)/guides/ios/troubleshooting/mtouch-errors/index.md
	cp binding_objc_libs.md $(WEBSITE_DOCS)/guides/cross-platform/macios/binding/objective-c-libraries/index.md
	cp binding_types_reference_guide.md $(WEBSITE_DOCS)/guides/cross-platform/macios/binding/binding-types-reference/index.md
	cp xamarin-ios-analysis.md $(WEBSITE_DOCS)/guides/ios/troubleshooting/xamarin-ios-analysis/index.md

diff:
	-diff -u $(WEBSITE_DOCS)/guides/ios/troubleshooting/mtouch-errors/index.md mtouch-errors.md
	-diff -u $(WEBSITE_DOCS)/guides/cross-platform/macios/binding/objective-c-libraries/index.md binding_objc_libs.md
	-diff -u $(WEBSITE_DOCS)/guides/cross-platform/macios/binding/binding-types-reference/index.md binding_types_reference_guide.md
	-diff -u $(WEBSITE_DOCS)/guides/ios/troubleshooting/xamarin-ios-analysis/index.md xamarin-ios-analysis.md

xamarin-analysis-doc:
	msbuild xamarin-analysis-doc-tool/xamarin-analysis-doc.sln
	mono $(XAMARIN_ANALYSIS_DOC)/xamarin-analysis-doc.exe $(XAMARIN_ANALYSIS)/Xamarin.iOS.Analysis.targets
