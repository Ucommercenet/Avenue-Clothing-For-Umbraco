'use strict';

const gulp = require('gulp'),
    sass = require('gulp-sass'),
    sourcemaps = require('gulp-sourcemaps'),
    autoprefixer = require('gulp-autoprefixer'),
    path = require('path'),
    gulpCopy = require('gulp-copy'),
    svgstore = require('gulp-svgstore'),
    svgmin = require('gulp-svgmin'),
    cheerio = require('gulp-cheerio'),
    eslint = require('gulp-eslint'),
    filter = require('gulp-filter'),
    browserSync = require('browser-sync').create();


// var gutil = require('gulp-util'),
// watch = require('gulp-watch'),
// notify = require('gulp-notify');

// gulp.task('hello-world', function(){
// console.log('My first gulp task');
// });

const basePath = 'src/AvenueClothing';

const config = {
    sassPath: basePath + 'sass',
    cssPath: basePath + 'css',
    jsPath: basePath + 'scripts',
    imagesPath: basePath + 'img'
}


// Sass
gulp.task('sass', function () {
    return gulp
        .src('./src/AvenueClothing/css/ucommerce-demostore/**/*.scss')
        .pipe(sass({
            outputStyle: 'compressed',
            precision: 10,
            includePaths: ['.'],
            onError: console.error.bind(console, 'Sass error:')
        }))
        .pipe(sourcemaps.init())
        .pipe(autoprefixer({
            grid: true,
            overrideBrowserslist: ['>1%, ie 11']
        }))
        .pipe(sourcemaps.write('.'))
        .pipe(filter('**/*.css'))
        .pipe(gulp.dest('C:/SQL Backups/UmbracoV8/css'))
        .pipe(gulp.dest('./src/AvenueClothing/css/'))
});

gulp.task('sprites', function () {
    return gulp
        .src(path.join(config.imagesPath, '/icons/*.svg'))
        .pipe(svgmin(function (file) {
            return {
                plugins: [{
                    cleanupIDs: {
                        minify: true
                    }
                }]
            }
        }))
        .pipe(cheerio({
            run: function ($) {
                $('[fill]').removeAttr('fill');
            },
            parserOptions: { xmlMode: true }
        }))
        .pipe(svgstore())
        .pipe(gulp.dest(config.imagesPath));
});

gulp.task('lint', function() {
    return gulp
        .src(config.jsPath)
        .pipe(eslint())
        .pipe(eslint.format());
    // Brick on failure to be super strict
    // .pipe(eslint.failOnError());
});

gulp.task('serve', function () {
    // Serve files from the root of this project
    browserSync.init({
        proxy: 'http://ac.local/'
    });
});

// Rerun the task when a file changes
gulp.task('watch', function () {
    gulp.watch('./src/AvenueClothing/css/ucommerce-demostore/**/*.scss', gulp.series('sass')).on("change", browserSync.reload);
    gulp.watch(config.imagesPath + '/**/*.svg', gulp.series('sprites')).on("change", browserSync.reload);
});

// gulp.task('default', gulp.parallel('watch', 'serve'));

gulp.task('default', gulp.parallel('watch'));


// //SASS
// gulp.task('sass', function() {
//     gulp.src('AvenueClothing/css/*.scss')
//         .pipe(sass().on('error', sass.logError))
//         .pipe(gulp.dest('AvenueClothing/css/'))
//         .pipe(notify({ message: 'Sass has been processed.' }));
// });

// gulp.task('watch-sass', function() {
//     gulp.watch('AvenueClothing/css/*.scss', ['sass']);  // If a file changes, re-run 'sass'
// });
