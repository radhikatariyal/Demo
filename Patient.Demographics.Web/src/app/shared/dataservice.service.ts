import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http';


@Injectable()
export class DataserviceService {
  BASE_URL="http://api.patient.localhost";
  private headers: Headers = new Headers({
    'Content-Type': 'application/x-www-form-urlencoded',
    'Accept': '*/*'
  });
  constructor(private httpclient: HttpClient) {   
  }
  getData(): Observable<any> {
    let url: string = this.BASE_URL + '/v1.0/accounts/getAllUsers';
    return this.httpclient.get(url, {responseType: 'text'});  
  }

  getJsonData(xmldata:any): Observable<any>{
    let parseString = require('xml2js').parseString;
    parseString( xmldata, function (err, result) {
      return result;
    });
    return  null;
  }


}
