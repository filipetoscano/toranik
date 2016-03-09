CssPipeline
=========================================================================

This module has four objectives:

* Automating the CSS authoring/build pipeline, so that once the developer
  has authored the CSS file it gets automatically built;

* Use of 'CSS Next', so that the developer can make use of latest CSS
  syntax, even if it is not currently supported in browsers, by
  transpiling into a version which is. Once browser support picks up,
  the transpiler does less and less.

* Bundling and minification into a single CSS file, as per bundle.css.
  This would represent the 'base' style package, used throughout the
  website. A rebuild of the bundle should be made if any of the parts
  changes.

* Individual minification of each partcss/*.css into seperate files.
  This would represent an individual part, used singularly in the website
  and which therefore does not get included into the (global) bundle.
  A singular build should be made whenever a part is changed.


Requirements
-------------------------------------------------------------------------

```
> npm install -g grunt-cli
> npm upgrade
```

Running
-------------------------------------------------------------------------

```
> grunt postcss:bundle        # Builds CSS bundle
> grunt postcss:part          # Builds individual CSS files
> grunt postcss               # Builds all output CSS files
> grunt watch                 # Watch the sources, builds matching target
```


Solution description
-------------------------------------------------------------------------

The moving parts of the solution are:

* `grunt`, as the (extensible) script runner, configured to meet the 
  aforementioned objectives with regards to the CSS build pipeline.
  Configuration of the tasks is done in the `Gruntfile.js` file;

* `postcss`, as the pluggable CSS build engine. In order to use PostCSS
  in Grunt, we rely on the `grunt-postcss` NPM plugin.

* A series of PostCSS processors, which affect the CSS files as they
  go through the pipeline. 

* `grunt-contrib-watch`, which adds watch functionality. Whenever a watched
  file is modified, the corresponding matching Grunt task is then executed;

* `grunt-newer`, which allows the solution to meet objective #4. Without


PostCSS: Used plugins
-------------------------------------------------------------------------

* `postcss-import`: Modifies the behavior of the @import at-rule, so that
  the referenced CSS files are inlined. This allows solution to meet
  objective #3, whereby the `bundle.css` file references all of the files
  which should be built together;

* `autoprefixer`: Automatically adds vendor prefixes to CSS properties.
  This keeps the source material leaner and much more readable. The value
  of `browsers` says what the target browser demographic is. Please
  consult [browserslist documentation](https://github.com/ai/browserslist)
  for reference and examples;

* `cssnano`: Minifies CSS content, but does not remove comments;

* `postcss-discard-comments`: Removes all comments, except those which
  start with an exclamation mark;

* `postcss-cssnext`: Allows use of standardized CSS syntax, but which is
  currently unsupported by browsers, meeting objective #2;

* `postcss-reporter`: Emits formatted errors to the console.



PostCSS: Additional plugins
-------------------------------------------------------------------------

There are many additional PostCSS plugins which can be used to further
customize the output. More can be found at: http://postcss.parts/

* [CSS optimizations](http://cssnano.co/optimisations/)
  A link to many `PostCSS` processors, with examples.

* [styleling](http://stylelint.io/)
  Linter for CSS.

* [postcss-cachebuster](https://github.com/glebmachine/postcss-cachebuster)
  Suffixes a query string to inline url() based on the last modified
  date of a file, thereby ensuring that stale files are not pulled from
  cache.

* [postcss-short](https://github.com/jonathantneal/postcss-short)
  Use a compact property, which is then fully expanded into it's
  parts.

/* eof */