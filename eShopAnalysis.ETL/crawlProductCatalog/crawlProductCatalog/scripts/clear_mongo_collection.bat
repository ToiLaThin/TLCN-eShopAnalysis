rem This script is used to clear all documents in ProductCollection
echo pass in the connection string as the first argument(remember to wrap in "") && echo then pass in the database name as the second argument && echo then pass in the collection name as the third argument
mongosh %1 --eval "db.getSiblingDB('%2').%3.deleteMany({})"
echo "Done"
pause