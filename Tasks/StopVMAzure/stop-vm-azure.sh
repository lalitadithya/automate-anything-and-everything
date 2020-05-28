#! /bin/bash

az login

az vm stop --resource-group "${ResourceGroupName}" --name "${VMName}"