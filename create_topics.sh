#!/bin/bash

# "topic-name:partitions:delete.retention.ms:cleanup.policy:retention.ms"
topics=(
    "oms-core-audit-trail-commands:12:604800000:delete:604800000"
    "oms-core-audit-trail-private-commands-db:12:604800000:delete:604800000"
    "oms-core-audit-trail-private-commands-s3:12:604800000:delete:604800000"
)

for topic in ${topics[@]}; do
   $(echo $topic | awk -F ":" '{print "/opt/bitnami/kafka/bin/kafka-topics.sh --bootstrap-server oms.kafka:9093 --topic " $1 " --create --if-not-exists --partitions " $2 " --replication-factor 1 --config delete.retention.ms=" $3 " --config cleanup.policy=" $4 " --config retention.ms=" $5}')
done
