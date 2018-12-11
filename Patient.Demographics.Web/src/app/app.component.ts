import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DataserviceService } from './shared/dataservice.service';
import { parseString } from 'xml2js';
import { UserModel } from './shared/UserModel';
import { Jsonp } from '@angular/http';

import { FormGroup, FormBuilder, Validators } from '@angular/forms';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public  users:any;
   userlist:any;
   userform:FormGroup;
   genderlist=[{"name":"Male","value":0},{"name":"Female","value":1}]
  constructor(private dataService: DataserviceService,private formBuilder: FormBuilder){
    this.userlist=[];
    this.dataService.getData().subscribe(
      res => {
        // this.dataService.getJsonData(res).subscribe(result=>{
          var result;
    var parser = require('xml2js');
    parser.Parser().parseString(res, (e, r) => {
      result = r.ArrayOfUserDto.UserDto
   
    }
    );
    this.users= result;
   // let jsonData = JSON.parse(this.users);  
    this.users.forEach(element => {
     // element.forEach(ele => {       
         console.log(element);
         let user:UserModel=new UserModel();
         user.DateOfBirth=element.DateOfBirth[0];
         user.FirstName=element.FirstName[0][0];
         user.LastName=element.LastName[0];
         user.Gender=element.Gender[0];
         if(element.HomeNumber!=undefined)
         user.HomeNumber=element.HomeNumber[0];
         if(element.WorkNumber!=undefined)
         user.WorkNumber=element.WorkNumber[0];
         if(element.MobileNumber!=undefined)
         user.MobileNumber=element.MobileNumber[0];
      this.userlist.push(user);
      
      //})
    });
          
      //  })
       
        //console.log(res);
      }
    );
  }
  ngOnInit() {

    this.userform = this.formBuilder.group({
      firstName: [null,[ Validators.required]],
      lastName: [null, Validators.required],
      gender: [null, [Validators.required]],      
      homeNumber: [null,[Validators.required]],
      workNumber: [null,[]],
      mobileNumber: [null,[Validators.required]],
      dateOfBirth: [null,[Validators.required]]
      /* ,
      NamingConvention: [null, [Validators.required]]
 */
    });
  }
}
