TOP=../../..
WEBSITE_DOCS ?= $(TOP)/xamarin-docs-pr
XAMARIN_ANALYSIS ?=$(TOP)/xamarin-analysis/shared/Xamarin.Analysis
XAMARIN_ANALYSIS_DOC = xamarin-analysis-doc-tool/bin/Debug

all: xamarin-analysis-doc
	cp mtouch-errors.md $(WEBSITE_DOCS)/docs/ios/troubleshooting/mtouch-errors.md
	cp mmp-errors.md $(WEBSITE_DOCS)/docs/mac/troubleshooting/mmp-errors.md
	cp binding_objc_libs.md $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/objective-c-libraries.md
	cp binding_types_reference_guide.md $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/binding-types-reference.md
	cp xamarin-ios-analysis.md $(WEBSITE_DOCS)/docs/ios/troubleshooting/xamarin-ios-analysis.md
	cp optimizations.md $(WEBSITE_DOCS)/docs/cross-platform/macios/optimizations.md

diff:
	-diff -u $(WEBSITE_DOCS)/docs/ios/troubleshooting/mtouch-errors.md mtouch-errors.md
	-diff -u $(WEBSITE_DOCS)/docs/mac/troubleshooting/mmp-errors.md mmp-errors.md
	-diff -u $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/objective-c-libraries.md binding_objc_libs.md
	-diff -u $(WEBSITE_DOCS)/docs/cross-platform/macios/binding/binding-types-reference.md binding_types_reference_guide.md
	-diff -u $(WEBSITE_DOCS)/docs/ios/troubleshooting/xamarin-ios-analysis.md xamarin-ios-analysis.md
	-diff -u $(WEBSITE_DOCS)/docs/cross-platform/macios/optimizations.md optimizations.md

xamarin-analysis-doc:
	msbuild xamarin-analysis-doc-tool/xamarin-analysis-doc.sln
	mono $(XAMARIN_ANALYSIS_DOC)/xamarin-analysis-doc.exe $(XAMARIN_ANALYSIS)/Xamarin.iOS.Analysis.targets
