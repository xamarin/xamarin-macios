TOP=../../..
WEBSITE_DOCS ?= $(TOP)/documentation

diff: 
	-diff -u $(WEBSITE_DOCS)/guides/ios/troubleshooting/mtouch-errors/index.md mtouch-errors.md
	-diff -u $(WEBSITE_DOCS)/guides/ios/advanced_topics/binding_objective-c/binding_objc_libs/index.md binding_objc_libs.md
	-diff -u $(WEBSITE_DOCS)/guides/ios/advanced_topics/binding_objective-c/binding_types_reference_guide/index.md binding_types_reference_guide.md 

all:
	cp mtouch-errors.md $(WEBSITE_DOCS)/guides/ios/troubleshooting/mtouch-errors/index.md
	cp binding_objc_libs.md $(WEBSITE_DOCS)/guides/ios/advanced_topics/binding_objective-c/binding_objc_libs/index.md
	cp binding_types_reference_guide.md $(WEBSITE_DOCS)/guides/ios/advanced_topics/binding_objective-c/binding_types_reference_guide/index.md 
	
