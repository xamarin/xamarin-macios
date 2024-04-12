TOP=../../../..
include $(TOP)/Make.config

run-tests run-unit-tests:
	$(Q_GEN) pwsh -Command "Install-Module -AcceptLicense -Force -AllowClobber Pester;Invoke-Pester"
