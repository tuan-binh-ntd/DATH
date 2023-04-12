export function checkResponseStatus(response: any): boolean{
    if(response && response.statusCode === 200 && response.data){
        return true;
    }
    else return false;
}