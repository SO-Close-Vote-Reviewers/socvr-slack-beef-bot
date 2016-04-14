
node('swarm') {
	stage 'checkout'
	checkout scm
	
	stage 'build docker image'
	sh "docker build ."
}
