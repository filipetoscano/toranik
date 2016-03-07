module.exports = function ( grunt ) {

    grunt.initConfig( {
        uglify: {
            options: {
                sourceMap: true,
                preserveComments: 'some',
                mangle: {
                    except: ['jQuery']
                }
            },
            bundle: {
                src: 'js/*.js',
                dest: 'bundle.min.js'
            },
            part: {
                files: [{
                    expand: true,
                    src: ['partjs/*.js', '!partjs/*.min.js'],
                    dest: '.',
                    ext: '.min.js'
                }]
            }
        },

        watch: {
            jsbundle: {
                files: ['js/*.js'],
                tasks: ['uglify:bundle']
            },

            jspart: {
                files: ['partjs/*.js', '!partjs/*.min.js'],
                tasks: ['uglify-part']
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
    grunt.loadNpmTasks( 'grunt-contrib-uglify' );
    grunt.loadNpmTasks( 'grunt-newer' );

    grunt.registerTask( 'uglify-part', ['newer:uglify:part'] );
}