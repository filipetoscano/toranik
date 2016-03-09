module.exports = function ( grunt ) {

    grunt.initConfig( {
        sprite: {
            /* one sprite target, per sprite set */
            seta: {
                src: 'img/sprite-a/*.png',
                dest: 'img/xsprites/a.png',
                retinaSrcFilter: 'img/sprite-a/*@2x.png',
                retinaDest: 'img/xsprites/a@2x.png',

                cssOpts: {
                    cssSelector: function ( sprite ) {
                        return '.icon-' + sprite.name;
                    },
                },

                destCss: 'css/sprites-a.css'
            }
        },

        watch: {
            /* one watch target, per sprite set */
            sprite_seta: {
                files: ['img/sprite-a/*.png'],
                tasks: ['sprite:seta']
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
    grunt.loadNpmTasks( 'grunt-spritesmith' );
}