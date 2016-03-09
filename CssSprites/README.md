CssSprites
=========================================================================

This proof has the following objectives:

* Automate the bundling of sprites into a single image file;
* Generate the corresponding CSS file;
* Reduce maintenance by avoiding double work (ie, by having to maintain
  the sprites themselves and a matching reference somewhere else);
* Fit into the CSS pipeline, as described in CssPipeline proof.


Requirements
-------------------------------------------------------------------------

```
> npm install -g grunt-cli
> npm upgrade
```


Running
-------------------------------------------------------------------------

```
> grunt sprite:seta             // Generates only sprite A
> grunt sprite                  // Generates all sprites
> grunt watch                   // Monitors changes, runs as necessary
```


Solution description
-------------------------------------------------------------------------

The moving parts of the solution are:

* grunt, as the (extensible) script runner. Configuration of the tasks
  is done in the Gruntfile.js file;

* grunt-spritesmith, which exposes 'spritesmith' as a Grunt task, which
  does all of the work.

/* eof */