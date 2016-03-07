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
