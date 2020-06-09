# Automate Anything and Everything

A simple, easy to use, cloud native and serverless automation framework built on Azure. It makes use of Azure Functions, Azure Container Instances, Azure Logic Apps, Bot Framework and more. 

## Architecture 
![Alt text](/Diagrams/ArchitectureDiagram.png?raw=true "Architecture Diagram")

## Worker Images
| Worker Image Name | Worker Image Description | Status |
|-------------------|--------------------------|--------|
| Az CLI | Docker images that contains the Azure CLI. This image can be used to perform automation tasks in Azure | ![Az CLI Docker Push](https://github.com/lalitadithya/automate-anything-and-everything/workflows/Az%20CLI%20Docker%20Push/badge.svg) |

## Components
| Component Name | Component Description | Status |
|----------------|-----------------------|--------|
| Task Orcestrator | This is an Azure functions app that takes care of deploying the worker images to the Azure Container Instances for a  task | ![Deploy-Azure-Functions](https://github.com/lalitadithya/automate-anything-and-everything/workflows/Deploy-Azure-Functions/badge.svg?branch=master) |
| Chat Bot | This is a chat bot that is built on top of Bot Framework v4 and it is deployed to Azure app service | ![Deploy Chat Bot to Azure Web Apps](https://github.com/lalitadithya/automate-anything-and-everything/workflows/Deploy%20Chat%20Bot%20to%20Azure%20Web%20Apps/badge.svg) |

## Contribute
This is in active development. Please feel free to fork and open PR. The road map can be found in Github issues. 
