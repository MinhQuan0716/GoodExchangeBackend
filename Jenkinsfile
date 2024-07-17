pipeline{
  agent any 
     tools {
           dotnetsdk "7.0" 
           }
      stages {
        stage('Checkout'){
          steps{
          git branch: 'main', credentialsId: '17f8e4c7-0641-406a-9115-b05cd1c57358', url: 'https://github.com/Goods-Exchange/BackendAPIProject'
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
