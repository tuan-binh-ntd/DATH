export function checkResponseStatus(response: any): boolean{
    if(response && response.statusCode >= 200 && response.statusCode < 300  && response.data){
        return true;
    }
    else return false;
}