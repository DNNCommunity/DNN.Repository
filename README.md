# DNN.Repository
DotNetNuke® Repository Module (RM)

The DotNetNuke® Repository Module (RM) can be used to store a collection of files, images, links or text on a server running the DotNetNuke® framework. While RM can simply be used as an enhanced Documents Module, it can also be used as much more. Through it’s use of a powerful and flexible template system to control the display and maintenance of items, you can use RM to provide file downloads to your visitors, or present a resource directory as a collection of links, or a collection of business cards to provide a business directory, or an image gallery or portfolio, or even a video library with an embedded inline video player.

RM is not limited to file uploads/downloads. You can even use RM for articles, news or a simple blog. The uses of RM are only limited by your imagination.

In addition to providing you with the ability to upload and download items, RM also provides a User Rating and Feedback system. You can decide if you want to allow your visitors to rate your items or enter comments on each item. RM also institutes a comprehensive security layer that allows you complete control over who can download, who can upload, who can rate or comment and where your items will be physically located on your web server. 

## Compatibility
| Dnn Repository version         | Min Tested Dnn version | Max Tested Dnn version |
| -------------------------:| ----------------------:| ----------------------:|
|      Included or 03.05.06 |               00.00.00 |               09.01.01 |
|                  04.00.00 |               06.01.00 |               09.02.00 |

## Building the module from source and submitting pull requests
1. Install Dnn (latest stable version) from https://github.com/dnnsoftware/Dnn.Platform/releases
2. Fork this repository to your own github account
3. Clone your fork to the Dnn DesktopModules folder
4. Important, the project name id Dnn.Repository, but the deployment directory is just Repository (more later)
5. Build the project in release mode using Visual Studio, this will create the installable packages in the Install folder of Dnn.Repository folder
6. Install one of the zip packages using the Dnn extension installer as any other module
7. In Visual Studio, create a new branch to isolate your changes.
8. In Visual studio, to test any changes, you need to build in debug mode, this will compile and copy all files from Dnn.Repository (the source code) to Repository (the deployment folder). To debug, use the attach to process feature and attach it to the w3wp process that matches the running site.
9. Commit and push your changes with clear descritions, then in github, create a pull request from the branch you created to the Dnn.Community repository, again please add a good description of the changes. You can also mention issues with #issueNumber to automatically associate your pull request with existing issues.