---
title: #Required; page title displayed in search results. Include the word "quickstart", and the brand.
description: #Required; article description that is displayed in search results. Include the word "quickstart".
author: #Required; your GitHub user alias, with correct capitalization.
ms.author: #Required; microsoft alias of author; optional team alias.
ms.service: #Required; service per approved list. service slug assigned to your service by ACOM.
ms.topic: quickstart #Required
ms.date: #Required; mm/dd/yyyy format.
---

<!---

IMPORTANT NOTE: Remove all the comments and example text in this template before you sign-off or merge to master.

What is a quickstart? 

    quickstarts are fundamental day-1 instructions for helping new
    customers use a subscription to quickly try out a specific product/service.
    The entire activity is a short set of steps that provides an initial experience.
    You only use quickstarts when you can get the service, technology, or
    functionality into the hands of new customers in less than 10 minutes.

Title Requirements:
    * Starts with "Quickstart: "
    * Make the first word following "quickstart:" a verb.
--->
# Quickstart: <do scenario> with the <service name> SDK for <language>


<!---
Introductory paragraph requirements:

    Lead with a light intro that describes, in customer-friendly language,
    what the customer will learn, or do, or accomplish. Answer the fundamental
    "why would I want to do this?" question.
    See the example quickstart intro below. 

IMPORTANT NOTES: 
    * Always link to the sample on GitHub 
    * Avoid notes, tips, and important boxes. Readers tend to skip over them.
      It's better to put that info directly into the article text.
--->

Use this quickstart to make your first image search using the Bing Image Search SDK for Node.js, 
which is a wrapper for the REST API and contains the same features. This JavaScript 
application sends an image search query, parses the JSON response, and displays the 
URL of the first image returned. While Bing Image Search has a REST API compatible 
with most programming languages, the SDK provides an easy way to integrate the service 
into your applications. The source code for this sample can be found on GitHub.

<!--- 
Prerequisites are the first H2 in a quickstart.

If there’s something a customer needs to take care of before they start (for
example, creating a VM, installing a package) it’s OK to link to that content before they begin.  
--->

## Prerequisites

<!-- Use these to start the prerequisites section, and modify as needed-->

<!--Example prerequisite section - Node.js -->
- The latest version of [Node.js](https://nodejs.org/)
- The [JavaScript Request library](https://github.com/request/request)
- The [<service > SDK for Node.js](link-to-sdk on npmjs.com) 
- Fourth prerequisite



<!--Example prerequisite section - Python -->
- [Python](https://www.python.org/) 2.x or 3.x
    
    It is recommended to use a virtual environment for your python development. You can install and initialize the virtual environment with the venv module. You can create a virtual environment with:
    ```console
    python -m venv mytestenv
    ```

- The [<service> SDK for Python](link-to-sdk-on pypi.org)

- Third prerequisite



<!--Example prerequisite section - C# -->

- Any edition of [the Visual Studio IDE](https://visualstudio.microsoft.com/vs/)
- The <service> SDK as a [NuGet package](link-to-nuget-package)
- Third prerequisite

<!--Example prerequisite section - Java

NOTE: 
    link to the search.maven.org page, as it includes the dependency information 
    for multiple build tools (like kotlin groovy, maven, etc.) and .jar downloads.

    Additionally, don't include the version number in the URL. By leaving it out, the site will redirect to the dependency's latest version.

    For example, this url:
        https://search.maven.org/artifact/com.microsoft.azure.cognitiveservices/azure-cognitiveservices-textanalytics/1.0.2-beta/jar
    Becomes this:
        https://search.maven.org/artifact/com.microsoft.azure.cognitiveservices/azure-cognitiveservices-textanalytics

    To find the SDK on this site:
    1. Use the following string in the search box:
        com.microsoft.azure.cognitiveservices.azure-cognitiveservices-
    2. start typing the name of your service to find it in the dropdown box. 
-->

- [The Java Development Kit(JDK) 8 or later](https://www.oracle.com/technetwork/java/javase/downloads/index.html)
- [The <service> SDK for Java](link-to-package-on search.maven.org)
- A Java project/dependency manager of your choice. For example:
    - [Apache Maven](https://maven.apache.org/)
    - [Gradle](https://gradle.org/) 
- Fourth prerequisite

<!--- Sign-up requirements:

    The Sign-up requirements inform customers that they need an active Cognitive Services subscription, 
    and gives them links for getting one, or a free trial key. Use the example below to create 
    these requirements.
    
    Links to include:
        Cognitive Services account creation article and API "try" page:
            * https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account
        Cognitive Services API "try" page
            * https://azure.microsoft.com/try/cognitive-services/
    
            Try to use the specific ?api= for your service in the second URL, if possible. 
            For example:
                https://azure.microsoft.com/try/cognitive-services/?api=bing-web-search-api


        The following links show customers where to get their keys on the azure portal and website after signing up.

            * location of the free trial key, after it's been activated
                https://azure.microsoft.com/try/cognitive-services/my-apis
            * Instructions for finding the key in the Azure Portal
                https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account#access-your-resource

See the example sign-up paragraph below
--->

You must have a [Cognitive Services API](https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account) subscription with access to the Text Analytics API. 
If you don't have a subscription, you can [create an account](https://azure.microsoft.com/try/cognitive-services/?api=bing-web-search-api) for free. Before continuing, 
you will need the service subscription key provided after activating your account. You can get your subscription key from the [Azure portal](https://docs.microsoft.com/azure/cognitive-services/cognitive-services-apis-create-account#access-your-resource) or [Azure website](https://azure.microsoft.com/try/cognitive-services/my-apis) after creating your account.

<!-- Language/Service specific setup instructions
    
    The next section guides customers through their first steps creating the application. 
    These include steps for importing any packages/dependencies, and any specific setup steps for the service.

        * Use an h2 (##) heading for "Creating a new application", and h3 (###) headings for the different aspects of 
        * For C# articles, recommend using the Visual Studio IDE. 
            For other languages, you can start this section with "Create a new <language> file/project in your favorite IDE or editor".
--->

## Create a new <language/technology> application

<!-- The following examples are separated by language to give you an idea of how to format these introduction steps for your quickstart.-->

<!-- C# example-->
### Create a .NET Core project

1. In the Visual Studio IDE, create a new project. In the **Visual C#** section, select **Console App (.NET Core)**.
C#
2. Enter a project name, leave the remaining default values, and click **OK**. By default, the primary code file named `Program.cs`.

### Install the NuGet SDK Package

1. Right-click on your project From the **Solution Explorer**, and select **Manage NuGet Packages..** from the menu. Click **Browse** and search for `<nuget-package-name>`. Click **Enter** to install the package.

    Installing the NuGet package also installs the following:
    
        * `Microsoft.Rest.ClientRuntime`
        * `Microsoft.Rest.ClientRuntime.Azure`
        * `Newtonsoft.Json`

2. Add the following `using` statements to your main code file.
    
    ```C#
    using Microsoft.Azure.CognitiveServices.Search.VisualSearch;
    using Microsoft.Azure.CognitiveServices.Search.VisualSearch.Models;
    ```

<!-- Node.js example-->
### Install the SDK package

1. After you've installed Node.js, create a new JavaScript file in your favorite IDE or editor.
2. Install the SDK and other dependencies with the following commands:
    1. ```console
        npm install ms-rest-azure
        ```
    2. ```console
        npm install azure-cognitiveservices-search-visualSearch
        ```
3. Add the following `require` statements to your main code file
    ```javascript
    const os = require("os");
    const async = require('async');
    const fs = require('fs');
    const Search = require('azure-cognitiveservices-visualsearch');
    ```

<!-- Python example -->
### Install the SDK package

1. After installing Python and optionally a virtual environment, create a new file in your favorite IDE or editor. 
2. Install the SDK with the following command:
    
    ```console
    pip install azure-cognitiveservices-search-imagesearch
    ```

<!-- Java example -->
### Install the SDK package

1. After installing Java, create a new project in your favorite IDE or editor. 
2. Install the SDK with a Java project management tool of your choice. Use the link to the SDK in the prerequisites section to find the dependency information for your tool.

You can also 
<!--
Task requirements:
    Tasks are the logical components of the quickstart scenario, (for example creating a service client, loading in data, etc. ). 
    Try to break tasks into manageable steps. For each step in a task, include a sentence or two to explain what is needed to complete it. 
    Use function names and and other specifics when necessary to add clarity.

    Some things to remember: 
        * Don't link off to other content - include whatever the customer needs to
        complete the scenario in the article. For example, if the customer needs
        to set permissions, include the permissions they need to set, and any specific
        settings. 

        * Don't link to reference topics in the procedural
        part of the quickstart when using cmdlets or code. Provide customers what they
        need to know in the quickstart to successfully complete the quickstart.
        
        * For portal-based tasks, minimize bullets and numbering, and for the CLI or PowerShell based tasks, don't use bullets or numbering.

Screenshot requirements:
    * Use screenshots but be judicious to maintain a reasonable length. Make
    sure screenshots align to the
    [current standards](https://review.docs.microsoft.com/help/contribute/contribute-how-to-create-screenshot?branch=master).
    If users access your product/service via a web browser the first screenshot
    should always include the full browser window in Chrome or Safari. This is
    to show users that the portal is browser-based - OS and browser agnostic.

Code requirements: 
    * Code requires specific formatting. Here are a few useful examples of
    commonly used code blocks. Make sure to use the interactive functionality where
    possible. For the CLI or PowerShell based tasks, don't use bullets or numbering. 
    You can add syntax highlighting by adding the programming language name after the ``` for the code snippet. 
    
    You can find a list of supported languages here: 
        https://review.docs.microsoft.com/en-us/help/contribute/code-in-docs?branch=master#snippet-syntax-reference
--->

## Task 1

1. Include a sentence or two to explain only what is needed to complete the
task. Use function names and and other specifics when necessary to add 
clarity. After creating the steps to complete the task, add the related code block.

    ```java
    cluster = Cluster.build(new File("src/remote.yaml")).create();
    //...
    client = cluster.connect();
    //...
    ```

2. Include a sentence or two to explain only what is needed to complete the
task. Use function names and and other specifics when necessary to add 
clarity. After creating the steps to complete the task, add the related code block.

    ```java
    cluster = Cluster.build(new File("src/remote.yaml")).create();
    //...
    client = cluster.connect();
    //...
    ```
    
3. Include a sentence or two to explain only what is needed to complete the
task. Use function names and and other specifics when necessary to add 
clarity. After creating the steps to complete the task, add the related code block.
 
    ```java
    cluster = Cluster.build(new File("src/remote.yaml")).create();
    //...
    client = cluster.connect();
    //...
    ```

## Task 2

1. Include a sentence or two to explain only what is needed to complete the
task. Use function names and and other specifics when necessary to add 
clarity. After creating the steps to complete the task, add the related code block.

    ```java
    cluster = Cluster.build(new File("src/remote.yaml")).create();
    //...
    client = cluster.connect();
    //...
    ```

2. Include a sentence or two to explain only what is needed to complete the
task. Use function names and and other specifics when necessary to add 
clarity. After creating the steps to complete the task, add the related code block.

    ```java
    cluster = Cluster.build(new File("src/remote.yaml")).create();
    //...
    client = cluster.connect();
    //...
    ```
    
3. Include a sentence or two to explain only what is needed to complete the
task. Use function names and and other specifics when necessary to add 
clarity. After creating the steps to complete the task, add the related code block.
 
    ```java
    cluster = Cluster.build(new File("src/remote.yaml")).create();
    //...
    client = cluster.connect();
    //...
    ```

<!---
To avoid any costs associated with following the quickstart task, a Clean
up resources (with an H2 heading) should come just before Next steps.

Add any steps needed for stopping application resources, etc.
--->

## Clean up resources

If you're not going to continue to use this application, delete the application resources
with the following steps:

1. From the left-hand menu...
2. ...click Delete, type...and then click Delete

<!-- Add a link to the source code on Github below -->

## Get the sample on GitHub

If you'd like to download the source code for this quickstart, you can find it on [GitHub](link-to-github-source-code).


<!--- Next steps requirements:
Quickstarts should always have a Next steps H2 that points to the next logical
quickstart in a series, or, if there are no other quickstarts, to some other
cool thing the customer can do. 

* Create a single link in the blue box format should direct the customer to the next article. 
    * As shown below, use:
     > [!div class="nextstepaction"]
     > [Article Title goes here](article-location-here)
    * You can shorten the displayed title if the original is too long.

* Do not use a "More info section" or a "Resources section" or a "See also section". 
--->

## Next steps

> [!div class="nextstepaction"]
> [Next steps button](article-location-here.md)
>
> * [Other useful articles](article-location-here.md)