#!/bin/bash

awslocal configure set aws_access_key_id $AWS_ACCESS_KEY_ID --profile=localstack
awslocal configure set aws_secret_access_key $AWS_SECRET_ACCESS_KEY --profile=localstack
awslocal configure set region $DEFAULT_REGION --profile=localstack

export AWS_DEFAULT_PROFILE=localstack
export QUEUE_NAME=visits
export QUEUE_ARN='arn:aws:sqs:'$DEFAULT_REGION':000000000000:'$QUEUE_NAME
export DLQ_NAME=visits-dlq
export DLQ_ARN='arn:aws:sqs:'$DEFAULT_REGION':000000000000:'$DLQ_NAME
export TOPIC_NAME=tracking
export TOPIC_ARN='arn:aws:sns:'$DEFAULT_REGION':000000000000:'$TOPIC_NAME

awslocal sqs create-queue --queue-name $DLQ_NAME

awslocal sqs create-queue --queue-name $QUEUE_NAME --attributes '{"RedrivePolicy":"{\"deadLetterTargetArn\":\"'$DLQ_ARN'\",\"maxReceiveCount\":\"3\"}"}'

awslocal sns create-topic --name $TOPIC_NAME

awslocal sns subscribe --topic-arn $TOPIC_ARN --protocol sqs --notification-endpoint $QUEUE_ARN --attributes '{"RawMessageDelivery":"true","FilterPolicy":"{\"Type\":[\"Visit\"]}"}'