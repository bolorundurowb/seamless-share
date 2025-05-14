import { AppSource } from './enums';

export interface UserRes {
  id: string,
  firstName?: string;
  lastName?: string;
  emailAddress: string;
  joinedAt: string;
}

export interface AuthRes {
  accessToken: string;
  expiresAt: string;
  user: UserRes;
}

export interface GenericMessageRes {
  message: string;
}

export interface ShareRes {
  id: string;
  code: string;
  createdAt: string;
}

interface BaseItemRes {
  id: string;
  createdAt: string;
  expiresAt: string;
  updatedAt: string;
  source?: AppSource;
  deviceName?: string;
  appVersion?: string;
}

export interface FileRes extends BaseItemRes {
  url: string;
  metadata: {
    externalId: string;
    name: string;
    extension: string;
    sizeInBytes: number;
    mimeType: string;
    checksum?: string;
  };
}

export interface LinkRes extends BaseItemRes {
  url: string;
}

export interface TextRes extends BaseItemRes {
  content: string;
}
