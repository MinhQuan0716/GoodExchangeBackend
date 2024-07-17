pipeline{
  agent any 
     tools {
           dotnetsdk "7.0" 
           }
      stages {
        stage('Checkout'){
          steps{
          git branch: 'main', credentialsId: '292716ce-f355-47de-9b7c-1ef731a38929', url: 'https://github.com/Goods-Exchange/BackendAPIProject'
        }
        }
         stage('Restore solution'){
                  steps {
                        withDotNet(sdk:'7.0'){
                            dotnetRestore project: 'BackendAPI.sln'
                        }
                    }
              }  
        stage('Build solution') {
           steps {
              withDotNet(sdk: '7.0') { // Reference the tool by ID
               dotnetBuild project: 'BackendAPI.sln', sdk: '7.0',noRestore: true
             }
             }
            }
          stage('Test solution'){
            steps {
              withDotNet(sdk:'7.0'){
                dotnetTest noBuild: true, project: 'BackendAPI.sln', sdk: '7.0'
              }
            }
          }
        stage('Pull code '){
          steps{
            sshPublisher(publishers: [sshPublisherDesc(configName: 'capstone', 
                                                       transfers: [sshTransfer(cleanRemote: false, excludes: '', execCommand: './githubpull.sh', execTimeout: 120000, flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '', remoteDirectorySDF: false, removePrefix: '', sourceFiles: '')]
                                                       , usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false)])
          }
        }
    stage('Clean workspace'){
           steps{
             cleanWs()
           }
         }
         }
      post {
           success {
             echo 'Pull code from git server success'
            }
      }
   
}
