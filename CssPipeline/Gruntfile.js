module.exports = function ( grunt ) {

    grunt.initConfig( {
        postcss: {
            options: {
                map: {
                    inline: false
                },

                processors: [
                    require( 'postcss-import' )(),
                    require( 'postcss-discard-comments' )(),
                    require( 'autoprefixer' )( { browsers: 'last 2 versions' } ),
                    require( 'postcss-cssnext' )(),
                    require( 'cssnano' ),
                    require( 'postcss-reporter' )
                ]
            },

            bundle: {
                src: 'bundle.css',
                dest: 'bundle.min.css'
            },

            part: {
                files: [{
                    expand: true,
                    src: ['partcss/*.css', '!partcss/*.min.css'],
                    dest: '.',
                    ext: '.min.css'
                }]
            }
        },

        watch: {
            cssbundle: {
                files: ['css/*.css', 'bundle.css'],
                tasks: ['postcss:bundle']
            },

            csspart: {
                files: ['partcss/*.css', '!partcss/*.min.css'],
                tasks: ['postcss-part']
            },

            configFiles: {
                files: ['Gruntfile.js'],
                options: {
                    reload: true
                }
            }
        }
    } );

    grunt.loadNpmTasks( 'grunt-contrib-watch' );
    grunt.loadNpmTasks( 'grunt-postcss' );
    grunt.loadNpmTasks( 'grunt-newer' );

    grunt.registerTask( 'postcss-part', ['newer:postcss:part'] );
}