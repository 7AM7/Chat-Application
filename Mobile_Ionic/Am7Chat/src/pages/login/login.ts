import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams,LoadingController  } from 'ionic-angular';
import { usercreds } from '../../models/interfaces/usercreds';
import {RegisterPage} from '../register/register';
import { AuthProvider } from '../../providers/auth/auth';
import {PasswordresetPage} from '../passwordreset/passwordreset';
import {TabsPage} from '../tabs/tabs';
/**
 * Generated class for the LoginPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-login',
  templateUrl: 'login.html',
})
export class LoginPage {
  credentials = {} as usercreds;
  constructor(public navCtrl: NavController, public navParams: NavParams, public authservice: AuthProvider, public loadingCtrl: LoadingController,) {
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad LoginPage');
  }

  signin() {
    let loader = this.loadingCtrl.create({
      content: 'Please wait'
    });
    this.authservice.login(this.credentials).then((res: any) => {
      loader.dismiss();
      if (!res.code)
        this.navCtrl.setRoot(TabsPage);
      else
        alert(res);
        
    })
  }
  signup() {
    this.navCtrl.push(RegisterPage);
  }

  passwordreset() {
    this.navCtrl.push(PasswordresetPage);
  }
}
