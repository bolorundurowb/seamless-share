export interface LoginReq {
  emailAddress: string;
  password: string;
}

export interface RegisterReq {
  emailAddress: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface AddTextToShareReq {
  content: string;
}


export interface AddFileToShareReq {
  content: File;
}
