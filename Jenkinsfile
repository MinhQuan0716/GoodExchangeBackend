pipeline{
  agent any 
     tools {
           dotnetsdk "7.0" 
           }
      stages {
        stage('Checkout'){
          steps{
            git branch: 'main', credentialsId: 'e8212876-bdbb-4735-9f7a-668ee9df4a10', url: 'https://github.com/Goods-Exchange/BackendAPIProject.git'
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
        stage('Pull code to server'){
          steps{
            sshPublisher(publishers: [sshPublisherDesc(configName: 'CapstoneSever', transfers: [sshTransfer(cleanRemote: false, excludes: '', execCommand: './githubpull.sh', execTimeout: 120000, flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '', remoteDirectorySDF: false, removePrefix: '', sourceFiles: '')], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false)])
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
