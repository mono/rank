#!/bin/sh
GIT_DIR="/home/rupert/monogit"
OUTPUT_DIR="/home/rupert/rank/rank server/Output"
GET_RANK_DIR="/home/rupert/rank/getRank/bin/Release/"

echo "  ***  Running getRank.exe"
mono "${GET_RANK_DIR}"/getRank.exe -d "${OUTPUT_DIR}" -g "${GIT_DIR}"
echo "  ***  Done"

sleep 5
cd "${OUTPUT_DIR}"
find -iname "*~" | xargs -i rm -rv "{}"
echo "  ***  Pushing rank files to remote site"
scp -r "${OUTPUT_DIR}"/* mono-web@go-mono.com:go-mono/bananas
echo "  ***  Done"

