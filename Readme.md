# Welcome to the Avenue Clothing Demo Store for Ucommerce

## Looking to install Avenue-Clothing
Please note that this repository is meant to build the package and/or serve as reference code. If you are looking to get avenue-clothing installed in the first place, this is not the place to start. please download the official supported package from our website:

https://ucommerce.net/platform/download/

## Compatibility ##
Ucommerce 9.0.0 and higher, Umbraco 8 and higher.

## Folder Overview ##

Below is a quick overview of the important folders

* lib - the folder which includes the various references we use
* package - the location of the build package
* src - where the magic is
	* AvenueClothing - All the files for the Avenue Clothing (this is where you will need to spend most of your time)
	* AvenueClothing.Installer - All the logic we use to build the package and store programattically. The place to look if you're interested in learning more about using the Ucommerce API
* tools - the various build scripts and tools we need to make the package

## How Do I Build The Package and install? ##

If you want to customise the store in any way, it's really easy. If you are using Visual Studio, you just need to build and **everything** will be built for you **automatically**!

To use the *DemoStoreDeploy.cmd* script, just open a command prompt, go to directory to the {project root}\tools\deploy folder, and run the *DemoStoreDeploy.cmd*

Example:

	cd: D:\Avenue-Clothing-For-Umbraco\tools\deploy
	DemoStoreDeploy.cmd

After the script runs, you will recieve a message saying:

	"Package saved to package\packageName_packageVersion.zip"

..and your package is ready to be installed, through the Packages section of Umbraco's backoffice.

Thanks to Visual Studio's integration with NPM, node packages get installed automatically as well, when building the project, just like nuget packages do, however if you are not using Visual Studio, you can still run the 'npm install' command from command prompt.

## What's Inside? ##

**THE STORE**

We've based the store's code on the [Twitter Bootstrap](http://twitter.github.com/bootstrap/) framework with a couple of additional styles to layout some of the more specific aspects of the store. All customisations are stored in separate folders to make it really clear where we've made changes.

* Complete product catalogue -including example custom product definitions
* Support for [Rich Text Snippets](http://schema.org/) to give your products the best chance in the search engines
* Example Razor Helper scripts to make it easier for you to manage your store
* Complete checkout process
* Basic search functionality
* Order confirmation email
* Rating and review system
* Custom Umbraco document types, master pages and content
* ...and more!

**THE INSTALLER**

The installer contains a load of examples on how to use the Ucommerce API to create:

* Product Catalogs and Groups
* Price Groups
* Shipping Methods
* Payment Methods
* Categories
* Products
* Umbraco media -and joining it to a product
* ...and more!

## What's Next? ##

That's largely up to you. This project has been made completely Open Source under the standard Umbraco MIT License so feel free to fork and adapt it to your heart's content. If you've done something cool then make a pull request.

## Keep In Touch ##

Don't forget to let us know what you're up to.

- [Ucommerce website](http://ucommerce.dk/ "The most feature rich e-commerce package for Umbraco") ([www.ucommerce.dk](http://ucommerce.dk/ "The most feature rich e-commerce package for Umbraco"))
- [Ucommerce on twitter](https://twitter.com/ucommerce) ([@ucommerce](https://twitter.com/ucommerce"))
- [The Site Doctor on twitter](https://twitter.com/thesitedoc) ([@thesitedoc](https://twitter.com/thesitedoc"))
- [S�ren on twitter](https://twitter.com/publicvoid_dk) ([@publicvoid_dk](https://twitter.com/publicvoid_dk"))
- [Tim on twitter](https://twitter.com/timgaunt) ([@timgaunt](https://twitter.com/timgaunt"))



Produced with love for you by [The Site Doctor](http://www.thesitedoctor.co.uk/ "The most feature rich e-commerce package for Umbraco") for
[Ucommerce](http://ucommerce.dk/ "The most feature rich e-commerce package for Umbraco")


### Front end
- run the UmbracoV8 DB to serve up the site (remember to copy over any changes you make to view files, js etc)

Styles:
- `yarn install` to install dependencies
- `gulp sass` to compile styles
- or use default task `gulp` to watch sass folder for changes
The gulp Sass task copies the css file from the the Avenue-Clothing-For-Umbraco project over to the UmbracoV8 folder (the file paths will need to match your own project setup)
