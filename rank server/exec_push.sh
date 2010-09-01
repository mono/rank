#!/bin/sh
GIT_DIR="/home/rupert/monogit"
OUTPUT_DIR="/home/rupert/rank/rank server/Output"
GET_RANK_DIR="/home/rupert/rank/rank server"

echo "  ***  Running getRank.sh"
"${GET_RANK_DIR}"/getRank.sh -d "${OUTPUT_DIR}" -g "${GIT_DIR}"
echo "  ***  Done"

sleep 5
find -iname "*~" | xargs -i rm -rv "{}"
echo "  ***  Pushing rank files to remote site"
scp -r "${OUTPUT_DIR}"/* mono-web@go-mono.com:go-mono/rank
echo "  ***  Done"
