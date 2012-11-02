# Welcome to the uCommerce Razor Demo Store#

This is the first release and is still in very active development so please be careful if you plan to install it on an existing uCommerce installation please **back up first**!

## Folder Overview ##

Below is a quick overview of the important folders

* lib - the folder which includes the various references we use
* package - the location of the build package
* src - where the magic is
	* uCommerce.RazorStore - All the files for the Razor Store (this is where you will need to spend most of your time)
	* UCommerce.RazorStore.Installer - All the logic we use to build the package and store programattically. The place to look if you're interested in learning more about using the uCommerce API
* tools - the various build scripts and tools we need to make the package

## Installation Instructions ##

**The install process should now be completely automatic!** Just in case you have a previous release -or it doesn't for you, the manual steps you (used) to have to do are at the end of this readme.

## What's Inside? ##

**THE STORE**

We've based the store's code on the [Twitter Bootstrap](http://twitter.github.com/bootstrap/) framework with a couple of additional styles to layout some of the more specific aspects of the store. All customisations are stored in seperate folders to make it really clear where we've made changes.

* Complete product catalogue -including example custom product definitions
* Support for [Rich Text Snippets](http://schema.org/) to give your products the best chance in the search engines
* Example Razor Helper scripts to make it easier for you to manage your store
* Complete checkout process
* Basic search functionality
* Order confirmation email
* Example uses of the new uCommerce JavaScript API
* Rating and review system
* Custom Umbraco document types, master pages and content
* ...and more!

**THE INSTALLER**

The installer contains a load of examples on how to use the uCommerce API to create:

* Product Catalogs and Groups
* Price Groups
* Shipping Methods
* Payment Methods
* Categories
* Products
* Umbraco media -and joining it to a product
* ...and more!

## How Do I Build The Package? ##

We've popped a copy of the latest version of the package ready to go for you into the *packages* folder but if you want to customise the store in anyway it's really easy. If you're using Visual Studio you just need to build and **everything** will be built for you **automatically**!

If you're not using Visual Studio, we're working on a solution for you at the moment but you can use the *DemoStoreDeploy.cmd* script currently stored in the *\tools\Deploy* folder (you may need to update the paths).

## What's Next? ##

That's largely up to you. This project has been made completely Open Source under the standard Umbraco MIT License so feel free to fork and adapt it to your heart's content. If you've done something cool then make a pull request.

We've already got a few extensions in the works for you including:

- Basic stock management
- Search auto-complete which displays the product in the list
- Introduction on how to theme the store
- Google Analytics integration
- Example login

## Keep In Touch ##

Don't forget to let us know what you're up to.

- [uCommerce website](http://ucommerce.dk/ "The most feature rich e-commerce package for Umbraco") ([www.ucommerce.dk](http://ucommerce.dk/ "The most feature rich e-commerce package for Umbraco"))
- [uCommerce on twitter](https://twitter.com/ucommerce) ([@ucommerce](https://twitter.com/ucommerce"))
- [The Site Doctor on twitter](https://twitter.com/thesitedoc) ([@thesitedoc](https://twitter.com/thesitedoc"))
- [Søren on twitter](https://twitter.com/publicvoid_dk) ([@publicvoid_dk](https://twitter.com/publicvoid_dk"))
- [Tim on twitter](https://twitter.com/timgaunt) ([@timgaunt](https://twitter.com/timgaunt"))



Produced with love for you by [The Site Doctor](http://www.thesitedoctor.co.uk/ "The most feature rich e-commerce package for Umbraco") for 
[uCommerce](http://ucommerce.dk/ "The most feature rich e-commerce package for Umbraco")

## Known Issues##

If you spot anything please let us know. We're currently aware of the following issues:

* Checkout: The catalog tries to create a member in Umbraco (which it can't do as there's no group added)
* Product Page: The first item doesn't update the mini cart
* You can't select Express or Standard (Free) shipping methods as there's no price for EUR
* The order confirmation email just has a link in it and no other content
* The JavaScript service doesn't work if you're running on an unlicensed version of the store

## Previous Installation Process ##

1. Update */umbraco/ucommerce/configuration/core.config* and if it's missing add the following node: `<component id="ServiceStackApi" service="uCommerce.RazorStore.ServiceStack.AppHost, uCommerce.RazorStore" type="uCommerce.RazorStore.ServiceStack.AppHost, uCommerce.RazorStore" lifestyle="Singleton"/>` just before the closing *components* node
1. Update */config/UrlRewriting.config* and alter the destinationUrl for the following rewrites (Thanks to [@MartinBakmand](https://twitter.com/MartinBakmand/status/257890315388719104 ) for spotting this one): 
	1. *DefaultCategoryProductRewrite*: ~/catalog/product.aspx?catalog=$2&amp;category=$3&amp;product=$4
	1. *DefaultProductRewrite*: ~/catalog/product.aspx?catalog=$2&amp;product=$3
	1. *DefaultCategoryRewrite*: ~/catalog.aspx?catalog=$2&amp;category=$3
	1. *DefaultCatalogRewrite*: ~/catalog.aspx?catalog=$2
1. Edit your web.config file and make the following changes:
	1. Ensure you have "*~/ucommerceapi/*" in the list of reserved paths (appSettings --> umbracoReservedPaths)
	1. Add the ucommerceapi *location* node to the end of your web.config just before the closing configuration tag (check the web.config file in the root of the *uCommerce.RazorStore* folder)
