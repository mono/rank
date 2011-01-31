#/bin/sh

export LANG=en_US.utf8
export LC_CTYPE="en_US.utf8"
export LC_NUMERIC="en_US.utf8"
export LC_TIME="en_US.utf8"
export LC_COLLATE="en_US.utf8"
export LC_MONETARY="en_US.utf8"
export LC_MESSAGES="en_US.utf8"
export LC_PAPER="en_US.utf8"
export LC_NAME="en_US.utf8"
export LC_ADDRESS="en_US.utf8"
export LC_TELEPHONE="en_US.utf8"
export LC_MEASUREMENT="en_US.utf8"
export LC_IDENTIFICATION="en_US.utf8"

GIT_DIR="/home/rupert/monogit"
OUTPUT_DIR="/home/rupert/rank/rank-server/Output"
GET_RANK_DIR="/home/rupert/rank/getRank/bin/Release/"
TEMP_DIR="/home/rupert/rank/rank-server/htdocs/"

echo "  ***  Running getRank.exe"
mono "${GET_RANK_DIR}"/getRank.exe -d "${OUTPUT_DIR}" -g "${GIT_DIR}" -t "${TEMP_DIR}"
echo "  ***  Done"

sleep 5
cd "${OUTPUT_DIR}"
find -iname "*~" | xargs -i rm -rv "{}"
echo "  ***  Pushing rank files to remote site"
scp -r "${OUTPUT_DIR}"/* mono-web@go-mono.com:go-mono/bananas
echo "  ***  Done"

