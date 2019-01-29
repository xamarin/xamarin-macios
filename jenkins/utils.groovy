def reportGitHubStatus(commitHash, context, backref, statusResult, statusResultMessage) {
    step([
        $class: 'GitHubCommitStatusSetter',
        reposSource: [$class: "ManuallyEnteredRepositorySource", url: "https://github.com/xamarin/xamarin-macios"],
        commitShaSource: [$class: "ManuallyEnteredShaSource", sha: commitHash],
        contextSource: [$class: 'ManuallyEnteredCommitContextSource', context: context],
        statusBackrefSource: [$class: 'ManuallyEnteredBackrefSource', backref: backref],
        statusResultSource: [$class: 'ConditionalStatusResultSource', results: [[$class: 'AnyBuildResult', state: statusResult, message: statusResultMessage]]]
    ])
}

return this
