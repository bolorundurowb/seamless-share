import { Injectable } from '@angular/core';
import { environment } from '../environment';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AddTextToShareReq, FileRes, LinkRes, ShareRes, TextRes } from '../types';

@Injectable({ providedIn: 'root'})
export class ShareService {
  private baseUrl = environment.apiV1BaseUrl;

  constructor(private http: HttpClient) {
  }

  async getOwnedShare(): Promise<ShareRes> {
    return firstValueFrom(this.http.get<ShareRes>(`${this.baseUrl}/shares`));
  }

  async getShareByCode(code: string): Promise<ShareRes> {
    return firstValueFrom(this.http.get<ShareRes>(`${this.baseUrl}/shares/${code}`));
  }

  async createShare(): Promise<ShareRes> {
    return firstValueFrom(this.http.post<ShareRes>(`${this.baseUrl}/shares`, {}));
  }

  async getTextShares(shareId: string): Promise<TextRes[]> {
    return firstValueFrom(this.http.get<TextRes[]>(`${this.baseUrl}/shares/${shareId}/text`));
  }

  async addTextToShare(shareId: string, req: AddTextToShareReq): Promise<TextRes> {
    return firstValueFrom(this.http.post<TextRes>(`${this.baseUrl}/shares/${shareId}/text`, req));
  }

  async deleteTextFromShare(shareId: string, textId: string): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.baseUrl}/shares/${shareId}/text/${textId}`));
  }

  async getLinkShares(shareId: string): Promise<LinkRes[]> {
    return firstValueFrom(this.http.get<LinkRes[]>(`${this.baseUrl}/shares/${shareId}/links`));
  }

  async deleteLinkFromShare(shareId: string, linkId: string): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.baseUrl}/shares/${shareId}/links/${linkId}`));
  }

  async getFileShares(shareId: string): Promise<FileRes[]> {
    return firstValueFrom(this.http.get<FileRes[]>(`${this.baseUrl}/shares/${shareId}/files`));
  }

  async createFileToShare(shareId: string, file: File): Promise<FileRes> {
    const formData = new FormData();
    formData.append('content', file);
    return firstValueFrom(this.http.post<FileRes>(`${this.baseUrl}/shares/${shareId}/files`, formData));
  }

  async deleteFileFromShare(shareId: string, fileId: string): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.baseUrl}/shares/${shareId}/files/${fileId}`));
  }
}
