import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams, AlertController  } from 'ionic-angular';
import { UserProvider } from '../../providers/user/user';
import {LoginPage} from '../login/login';
/**
 * Generated class for the PasswordresetPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-passwordreset',
  templateUrl: 'passwordreset.html',
})
export class PasswordresetPage {
    email: string;
  constructor(public navCtrl: NavController, public navParams: NavParams, public alertCtrl: AlertController ,public userservice: UserProvider) {
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad PasswordresetPage');
  }

  reset() {
    let alert = this.alertCtrl.create({
      buttons: ['Ok']
    });
    this.userservice.passwordreset(this.email).then((res: any) => {
      if (res.success) {
        alert.setTitle('Email Sent');
        alert.setSubTitle('Please follow the instructions in the email to reset your password');
      }
    }).catch((err) => {
      alert.setTitle('Failed');
      alert.setSubTitle(err);
    })
  }

  goback() {
    this.navCtrl.setRoot(LoginPage);
  }
}
