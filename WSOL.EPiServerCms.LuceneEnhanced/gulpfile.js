/// <binding AfterBuild="" BeforeBuild="" ProjectOpened="" Clean="clean:packages" />

// global variables
var nugetServers = [
    "http://nuget.wsol.com", // public 
    "http://nuget.wsoldev.com", // dev
];

var customSlackConfig = {}; // to override channel, username, icon, or releasenotes path
var directory = "./_packages-nuget/";
var pushedFolder = directory;
var nugetPath = 'C:/_Web/Utils/nuget.exe';
var slackConfig = 'C:/_Web/Utils/slackconfig.json';
var gitConfigPath = "../.git/config";

// Uncomment the line of code below to enable external gulp tasks.
// Important: New tasks will require an update to the main gulpfile.js before Visual Studio can use it.
// Important: Bindings for external tasks must be set in main gulpfile.js
// Hint: just add a whitespace and save!
//var ext = require('./gulpfile.custom.js'); 

// dependencies
var gulp = require('gulp'),
    nuget = require('gulp-nuget'), // https://www.npmjs.com/package/gulp-nuget
    msbuild = require("gulp-msbuild"), // https://www.npmjs.com/package/gulp-msbuild
    mstest = require("gulp-mstest"),
    del = require("del"),
    mergeStream = require('ordered-merge-stream'),
    Slack = require('slack-node'),
    gitConfig = require('parse-git-config'),
    stripJsonComments = require('strip-json-comments')
fs = require("fs"); // read files;

// gets path to solution relative to gulpjs file
var solutionDir = process.cwd() + '\\..\\';

function cleanPackages() {
    return del([directory + '*.nupkg', "./TestResults/**/*"]);
}

function mergeInto(o1, o2) {
    if (o1 == null || o2 == null)
        return o1;

    for (var key in o2)
        if (o2.hasOwnProperty(key))
            o1[key] = o2[key];

    return o1;
}

gulp.task('default', ['slack:notify'], cleanPackages);

gulp.task('clean:packages', cleanPackages);

gulp.task('msbuild:test', ['msbuild:release'], function () {
    var testPath = "..//**/bin/[Rr]elease/*[Tt]est*.dll";

    return gulp.src(testPath).pipe(mstest({
        outputEachResult: true,
        quitOnFailed: true,
        errorStackTrace: false,
        errorMessage: true
    }));
});

gulp.task("msbuild:release", ["clean:packages"], function () {
    var options = {
        targets: ['Clean', 'Rebuild'],
        //stdout: true,
        properties: { Configuration: 'Release', SolutionDir: solutionDir },
        toolsVersion: 14
    };

    return gulp.src('../*.sln').pipe(msbuild(options));
});

gulp.task('nuget:pack', ['msbuild:test'], function () {
    var options = {
        nuget: nugetPath,
        outputDirectory: directory,
        properties: 'configuration=release',
        symbols: true
    };

    return gulp.src('./*.csproj').pipe(nuget.pack(options));
});

gulp.task('nuget:push', ["nuget:pack"], function () {
    var src = directory + '*.nupkg';
    var tasks = [];
    nugetServers.forEach(function (server) {
        var options = {
            nuget: nugetPath,
            source: server,
            timeout: '300',
        };

        tasks.push(
            gulp.src(src)
            .pipe(nuget.push(options))
        );
    });

    tasks.push(gulp.src(src).pipe(gulp.dest(pushedFolder)));

    return mergeStream(tasks);
});

gulp.task('slack:notify', ['nuget:push'], function () {
    var configSource = fs.readFileSync(slackConfig, { encoding: 'utf-8' });
    var config = JSON.parse(stripJsonComments(configSource).replace(/\s+/g, " "));

    if (config) {
        if (typeof customSlackConfig === 'undefined') {
            customSlackConfig = {};
        }

        config = mergeInto(config, customSlackConfig);
        var files = fs.readdirSync(pushedFolder);
        var releaseNotes = "";
        var attachments = [];

        try {
            releaseNotes = fs.readFileSync(config.releasenotes, { encoding: 'utf-8' });
        } catch (e) { };

        if (releaseNotes.length > 1) {
            attachments = [
                {
                    "fallback": releaseNotes,
                    "text": releaseNotes,
                    "title": "Release Notes"
                }
            ];
        }

        if (files.length > 0) {
            var git = gitConfig.sync({ path: gitConfigPath });
            var packageNameAndVersion = files[0].substr(0, files[0].lastIndexOf('.'));
            var slack = new Slack();
            slack.setWebhook(config.webhook);

            slack.webhook({
                channel: config.channel,
                username: config.username,
                icon_emoji: config.icon,
                text: packageNameAndVersion + " is now available! <" + git['remote "origin"'].url + "?_a=history| View History>",
                attachments: attachments
            }, function (err, response) {
                console.log(response);
            });
        } else {
            console.log("No package found to announce!");
        }
    } else {
        console.log("No config found, please add one for: " + slackConfig);
    }
});