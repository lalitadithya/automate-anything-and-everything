#! /bin/bash

wget -O script.sh $1
chmod a+x script.sh
./script.sh

if [ $? -eq 0 ]
then 
  curl -d '{"TaskStatus":true}' -H "Content-Type: application/json" -X POST "${CallbackURI}"
else
  curl -d '{"TaskStatus":false}' -H "Content-Type: application/json" -X POST "${CallbackURI}"
fi