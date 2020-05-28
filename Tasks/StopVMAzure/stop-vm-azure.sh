#! /bin/bash

az login --identity

az vm stop --resource-group "${ResourceGroupName}" --name "${VMName}"