# Project name : Augmented Sapiens 
## Project Description : AR Maintenance App, where you tag certain things using virtual markers (like a virtual post-it note) and store the images in the cloud for someone else to review.		
### Backend
Deployment steps for flask app onto Heroku :
- If you don’t have a Heroku account , sign up for a new account.
- Download and Install Heroku CLI.
- On the IDE of your choice , have your flask app ready. For example : <br>
[![flask1.png](https://i.postimg.cc/J0BZZ1s2/flask1.png)](https://postimg.cc/kRq2dPFQ)
- Open the terminal at this application location and run the following command. <br>
  pip3 install gunicorn.<br>
  pip3 freeze > requirements.txt <br>
  touch Procfile # creates a Procfile <br>
- Inside the procfile , add the following :<br>
  Web: gunicorn app:app
- Now go to gthub and create a new repository if it is not existing already for your application.
- Push the code to github repo.
- Next go to terminal and type : heroku login
- This will take you to the heroku login page. Click on login and return back to the terminal
- Create heroku app on terminal command: heroku create flask-app-deploy
- Once this is done run the following command to deploy our code to heroku.<br>
  git push heroku <branch_name>
- Once done , type heroku open . It will open the deployed app on the browser.
- We can also deploy an application through the Heroku page - New app → Under staging / production → New app → Fill in app name and click on create app → Go inside the app → Under Deploy , Deployment method → Connect the app to Github repository → Under Automatic / Manual Deploy , choose the branch you want to deploy and click on deploy




				
