AB_TARGET := ''
AB_OUTPUT_FOLDER_BASE := AssetBundles
AB_OUTPUT_FOLDER := $(AB_OUTPUT_FOLDER_BASE)/$(AB_TARGET)
UNITY_PATH := ''

.PHONY: help
#: ヘルプコマンド
help:
	@grep -A1 -E "^#:" Makefile \
	| grep -v -- -- \
	| sed 'N;s/\n/###/' \
	| sed -n 's/^#: \(.*\)###\(.*\):.*/\2###\1/p' \
	| column -t  -s '###'

.PHONY: clean-ab
#: AssetBundlesフォルダ消す
clean-ab:
	rm -rf $(AB_OUTPUT_FOLDER_BASE)

.PHONY: unity-build-ab-standalone
unity-build-ab-standalone:
	$(UNITY_PATH) -batchmode -quit -projectPath . -logFile - \
	-executeMethod AssetBundleBuilder.BatchBuild \
	-outputFolder $(AB_OUTPUT_FOLDER) \
	-target $(AB_TARGET)

.PHONY: rsync-to-docs
#: AssetBundlesフォルダをdocs以下にコピー
rsync-to-docs:
	rsync -ahvnc --exclude='*/buildlogtep.json' ./AssetBundles ./docs

.PHONY: format
format:
	dotnet format AssetBundleHubSample.sln
